import sys
import json
import spacy

nlp = spacy.load("fr_core_news_md")

def main():
    sentence = sys.stdin.read()
    doc = nlp(sentence.strip())
    results = []
    for token in doc:
        results.append({
            "text": token.text,
            "lemma": token.lemma_,
            "dep": token.dep_,
            "head": token.head.text,
            "pos": token.pos_
        })
    print(json.dumps(results, ensure_ascii=False))

if __name__ == "__main__":
    main()
