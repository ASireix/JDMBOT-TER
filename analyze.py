import sys
import json
import spacy

nlp = spacy.load("fr_core_news_md")

def main():
    try:
        sentence = sys.stdin.read().strip()
        doc = nlp(sentence)
        results = [{
            "text": token.text,
            "lemma": token.lemma_,
            "dep": token.dep_,
            "head": token.head.text,
            "pos": token.pos_
        } for token in doc]
        print(json.dumps(results, ensure_ascii=False))
    except Exception as e:
        print(f"PYTHON ERROR: {e}", file=sys.stderr)
        exit(1)

if __name__ == "__main__":
    main()
