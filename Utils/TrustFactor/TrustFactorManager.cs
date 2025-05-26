namespace BotJDM.Utils.TrustFactor;

public static class TrustFactorManager
{
    public static int threshold = -1;

    public static readonly int minTrust = -1;
    public static readonly int maxTrust = 1;
    
    public static float UpdateTrustFactor(float currentTrustFactor, bool increaseValue)
    {
        // Clamp entre -1 et 1
        currentTrustFactor = Math.Clamp(currentTrustFactor, -1f, 1f);

        float direction = increaseValue ? 1f : -1f;

        // Valeur maximale de mise à jour
        float learningRate = 0.05f;

        // Courbe d'atténuation qui ralentit vers les extrêmes, mais n'annule jamais : ici (1 - currentTrustFactor^2)
        float damping = 1f - currentTrustFactor * currentTrustFactor;

        // Calcul de l'incrément
        float delta = learningRate * damping * direction;

        // Mise à jour du trust factor
        float newTrustFactor = currentTrustFactor + delta;

        // Clamp final pour rester dans les bornes
        return Math.Clamp(newTrustFactor, minTrust, maxTrust);
    }

    public static int AdjustProbability(int baseProbability, float trustFactor, bool positiveInfluence)
    {
        // Clamp trustFactor et base proba
        trustFactor = Math.Clamp(trustFactor, -1f, 1f);
        baseProbability = Math.Clamp(baseProbability, 0, 100);

        // Détermine la direction d'influence :
        // true  → +trustFactor augmente la proba
        // false → +trustFactor diminue la proba
        float direction = positiveInfluence ? 1f : -1f;

        // Calcul de l'ajustement : linéaire entre -10 et +10
        float adjustment = trustFactor * 10f * direction;

        // Appliquer l'ajustement
        int adjustedProbability = (int)Math.Round(baseProbability + adjustment);
        return Math.Clamp(adjustedProbability, 0, 100);
    }


}