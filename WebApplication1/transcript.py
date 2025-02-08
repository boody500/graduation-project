import sys
import json     
from youtube_transcript_api import YouTubeTranscriptApi

from gensim.models import Word2Vec
from nltk.tokenize import word_tokenize
import nltk
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity




def fetch_transcript(video_id):
    try:
        transcript = YouTubeTranscriptApi.get_transcript(video_id)
        for element in transcript:
            element.update({"end" : element["start"]+element["duration"]})
        return transcript
    
    except Exception as e:
        return {""}

def best_match(video_id,prompt):
    # Step 2: Import required libraries

    transcript = fetch_transcript(video_id)


    if transcript == "":
        return {""}
     # Check if fetch_transcript returned an error
    if isinstance(transcript, dict) and "error" in transcript:
        return {"error": transcript["error"]}

    # Step 3: Prepare sample documents
    documents = [caption.get("text") for caption in transcript]
    start = [start.get("start") for start in transcript]
    end = [end.get("end") for end in transcript]
    durations = [duration.get("duration") for duration in transcript]
    documents.append(prompt)

    # Step 4: Tokenize the documents
    tokenized_documents = [word_tokenize(doc.lower()) for doc in documents]

    # Step 5: Train Word2Vec model using CBOW
    # sg=0 for CBOW, vector_size=50 specifies the embedding size
    model = Word2Vec(sentences=tokenized_documents, vector_size=50, window=3, min_count=1, sg=0) #sg = 0 for CBOW

    # Step 6: Get word embeddings
    word_embeddings = {word: model.wv[word] for word in model.wv.index_to_key}

    #calculate the avg of weights
    document_embeddings = []
    for doc in tokenized_documents:
        doc_embedding = np.mean([model.wv[word] for word in doc if word in model.wv], axis=0)
        document_embeddings.append(doc_embedding)

    cosine_sim = cosine_similarity(document_embeddings)

    # Drop the similarity with itself
    copy = cosine_sim[-1]  # The last row corresponds to the similarity with the prompt
    copy = copy[:-1]  # Exclude the self-similarity (the prompt compared to itself)

    # Get the top 10 highest cosine similarities
    top_10_indices = np.argsort(copy)[-10:][::-1]  # Sort and get the indices of the top 10

    # Return the top 10 documents and their corresponding cosine similarities
    best_documents_with_similarity = [{"text": documents[i], "cos_similarity": float(copy[i]), "start" : float(start[i]), "end" : float(end[i]), "duration" : float(durations[i])} for i in top_10_indices]

    return best_documents_with_similarity







if __name__ == "__main__":
    video_id = sys.argv[1]
    transcript = fetch_transcript(video_id)

    #get transcript only
    if(len(sys.argv) == 2):
        print(json.dumps(transcript))

    #find best match promot
    else:
        prompt = sys.argv[2]
        caption = best_match(video_id,prompt)
        print(json.dumps(caption))

            
