
ALTER TABLE public."file_collection" ADD COLUMN auto_faiss_sync BOOLEAN NOT NULL DEFAULT FALSE;


CREATE TABLE public."global_collection_config"(
    auto_faiss_sync BOOLEAN NOT NULL DEFAULT FALSE
);