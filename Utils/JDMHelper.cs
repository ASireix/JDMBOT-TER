using System.Text.Json;
using System.Text.Json.Serialization;
using BotJDM.APIRequest;
using BotJDM.APIRequest.Models;
using BotJDM.Database.Entities;
using BotJDM.Database.Mappers;
using BotJDM.Database.Services;
using BotJDM.Utils.TrustFactor;

namespace BotJDM.Utils;

public static class JDMHelper
    {
        private static string _cachePath;
        private static string _relationsTypesPath;
        private static int _maxCacheSize;
        private static RelationService _relationService;
        private static NodeService _nodeService;
        private static UserService _userService;
        private static int maxDepth = 8;

        public static async Task Initialize(string cachePath, int maxCacheSize, string relationsTypesPath, RelationService relationService
        , NodeService nodeService, UserService userService)
        {
            _cachePath = cachePath;
            _maxCacheSize = maxCacheSize;
            _relationService = relationService;
            _nodeService = nodeService;
            _userService = userService;
            _relationsTypesPath = relationsTypesPath;
            var relations = await JDMApiHttpClient.GetRelationTypes();
            Console.Write("Acquired relations types for JDMHelper initialization");
            var json = JsonSerializer.Serialize(relations);
            Console.WriteLine("Chemin du fichier JSON : " + _relationsTypesPath);
            await File.WriteAllTextAsync(_relationsTypesPath, json);
        }

        public static async Task DisplayRelationsTypes()
        {
            List<RelationType> relationsTypes = await JDMApiHttpClient.GetRelationTypes() ?? new List<RelationType>();
            Console.WriteLine($"There is {relationsTypes.Count} relations types");
            foreach (var type in relationsTypes)
            {
                Console.WriteLine($"Id : {type.id} | Name : {type.name}");
            }
        }

        /// <summary>
        /// Renvoie une relation et les deux bool de fin correspondent à depuis une inférence et depuis la base locale
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="type"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public static async Task<(Relation,bool,bool)?> FindRelationFromTo(string node1, string node2, string type, SearchType searchType = SearchType.Basique)
        {
            (Relation, bool, bool)? res = null;
            Console.WriteLine("Init");
            var typeId = GetTypeIdFromTypeName(type);
            
            //BD Check
            res = await CheckRelationFromBd(node1, node2, typeId, searchType);
            if (res != null)
            {
                return res;
            }

            //Cache Check
            res = await CheckRelationFromCache(node1, node2, typeId, searchType);
            if (res != null)
            {
                return res;
            }
                    
            //API Check
            res = await CheckRelationFromApi(node1, node2, typeId, searchType);
            if (res != null)
            {
                return res;
            }
            return res;
        }

        public static async Task UpdateUserTrustFactor(ulong discordUserId, string username, bool positive)
        {
            UserEntity user = await _userService.AddUserAsync(discordUserId, username);
            float newTrust = TrustFactorManager.UpdateTrustFactor(user.TrustFactor, positive);
            await _userService.UpdateTrustFactorAsync(discordUserId, newTrust);
        }

        public static async Task UpdateRelationInDatabase(string relationType, string node1, string node2, ulong discordUserId, string username, bool positive = true)
        {
            var relationTypeId = GetTypeIdFromTypeName(relationType);
            var user = await _userService.AddUserAsync(discordUserId, username);
            
            var potentialRel = await _relationService.RelationExistsBetweenAsync(node1,node2,relationTypeId, 0);
            if (potentialRel == null)
            {
                Node? _node1 = await JDMApiHttpClient.GetNodeByName(node1);
                Node? _node2 = await JDMApiHttpClient.GetNodeByName(node2);

                NodeEntity node1Entity;
                NodeEntity node2Entity;
            
                if (_node1 == null)
                {
                    node1Entity = new NodeEntity()
                    {
                        Name = node1,
                        New = true
                    };
                }
                else
                {
                    node1Entity = _node1.ToEntity();
                }
            
                if (_node2 == null)
                {
                    node2Entity = new NodeEntity()
                    {
                        Name = node2,
                        New = true
                    };
                }
                else
                {
                    node2Entity = _node2.ToEntity();
                }
            
                //ajoute les 2 noeuds
                node1Entity = await _nodeService.UpdateNode(node1Entity);
                node2Entity = await _nodeService.UpdateNode(node2Entity);

                RelationEntity relation = new RelationEntity()
                {
                    Probability = 50,
                    CreationDate = DateTime.Now,
                    Node1 = node1Entity.Id,
                    Node2 = node2Entity.Id,
                    Type = relationTypeId
                };
            
                //ajoute la relation

                potentialRel = await _relationService.AddRelationAsync(relation);
                
                var p = await _relationService.GetProbabilityAsync(potentialRel.Id);
                var newProba = TrustFactorManager.AdjustProbability(p ?? 0, user.TrustFactor, positive);
                await _relationService.SetProbabilityAsync(potentialRel.Id, newProba);
            }
            else
            {
                var p = await _relationService.GetProbabilityAsync(potentialRel.Id);
                var newProba = TrustFactorManager.AdjustProbability(p ?? 0, user.TrustFactor, positive);
                await _relationService.SetProbabilityAsync(potentialRel.Id, newProba);
            }
        }

        public static async Task FindBestQuestion()
        {
            
        }

        private static async Task<(Relation, bool, bool)?> CheckRelationFromBd(string node1, string node2, int typeId, SearchType searchType)
        {
            RelationEntity? relationEntity = null;
            bool transitivity = false;
            switch (searchType)
            {
                case SearchType.Basique:
                    relationEntity = await _relationService.RelationExistsBetweenAsync(node1, node2, typeId);
                    break;
                case SearchType.Inference:
                    relationEntity = await _relationService.CheckRelationOrTransitiveAsync(node1, node2, typeId, maxDepth);
                    transitivity = true;
                    break;
                case SearchType.Extensive:
                    break;
                default:
                    break;
            }
            
            return relationEntity != null ? (relationEntity.ToRelation(), transitivity, true) : null;
        }
        
        private static async Task<(Relation, bool, bool)?> CheckRelationFromCache(string node1, string node2, int typeId, SearchType searchType)
        {
            var cachedRelations = LoadCache();
            var cached = cachedRelations.FirstOrDefault(r => r.Node1Name == node1 && r.Node2Name == node2 && r.Relation.type == typeId);

            return cached != null ? (cached.Relation, false, false) : null;
        }

        private static async Task<(Relation, bool, bool)?> CheckRelationFromApi(string node1, string node2, int typeId,SearchType searchType)
        {
            var node1Api = await JDMApiHttpClient.GetNodeByName(node1);
            Console.WriteLine($"Node {node1} searched");
            var node2Api = await JDMApiHttpClient.GetNodeByName(node2);
            Console.WriteLine($"Node {node2} searched");
            RelationRet? relationRet = null;
                    
            if (node1Api == null || node2Api == null)
            {
                return null;
            }
            Console.WriteLine($"Found nodes : {node1} and  {node2}");
            
            switch (searchType)
            {
                case SearchType.Basique:
                    relationRet = await JDMApiHttpClient.GetRelationsFromTo(node1, node2, [typeId]);
                    break;
                case SearchType.Inference:
                    relationRet = await JDMApiHttpClient.GetRelationsFromTo(node1, node2, [typeId],transitivityDepth:maxDepth);
                    break;
                case SearchType.Extensive:
                    break;
                default:
                    break;
            }

            if (relationRet == null)
            {
                return null;
            }
                    
            var relationApi = relationRet.relations.FirstOrDefault(r => r.type == typeId);
            if (relationApi == null)
            {
                return null;
            }

            var cacheRelation = new CachedRelation
            {
                Node1Name = node1Api.name,
                Node2Name = node2Api.name,
                Node1Id = node1Api.id,
                Node2Id = node2Api.id,
                Relation = relationApi
            };
                    
            AddToCache(cacheRelation);
            return (relationApi,false,false);
        }

        public static List<RelationType> LoadRelationTypes()
        {
            if (!File.Exists(_relationsTypesPath)) return new List<RelationType>();
            var json = File.ReadAllText(_relationsTypesPath);
            return JsonSerializer.Deserialize<List<RelationType>>(json) ?? new List<RelationType>();
        }

        private static int GetTypeIdFromTypeName(string typename)
        {
            List<RelationType> relationTypes = LoadRelationTypes();
            if (relationTypes.Count == 0) return -1;
            int id = relationTypes.FirstOrDefault(r => r.name == typename).id;
            return id;
        }
        
        private static Queue<CachedRelation> LoadCache()
        {
            if (!File.Exists(_cachePath)) return new Queue<CachedRelation>();

            var json = File.ReadAllText(_cachePath);
            var list = JsonSerializer.Deserialize<List<CachedRelation>>(json) ?? new List<CachedRelation>();
            return new Queue<CachedRelation>(list);
        }
        
        private static void SaveCache(Queue<CachedRelation> cache)
        {
            var json = JsonSerializer.Serialize(cache.ToList());
            File.WriteAllText(_cachePath, json);
        }

        
        private static void AddToCache(CachedRelation newEntry)
        {
            var cache = LoadCache();

            if (cache.Count >= _maxCacheSize)
            {
                cache.Dequeue(); // Supprime le plus ancien
            }

            cache.Enqueue(newEntry);
            SaveCache(cache);
        }
    }

public enum SearchType
{
    Basique,
    Inference,
    Extensive
}

public class CachedRelation
{
    [JsonPropertyName("relation")]
    public Relation Relation { get; set; }

    [JsonPropertyName("node1_name")]
    public string Node1Name { get; set; }
    
    [JsonPropertyName("node1_id")]
    public int Node1Id { get; set; }

    [JsonPropertyName("node2_name")]
    public string Node2Name { get; set; }
    
    [JsonPropertyName("node2_id")]
    public int Node2Id { get; set; }
}