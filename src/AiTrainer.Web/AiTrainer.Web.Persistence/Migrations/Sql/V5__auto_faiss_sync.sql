
ALTER TABLE public."file_collection" ADD COLUMN auto_faiss_sync BOOLEAN NOT NULL DEFAULT FALSE;


CREATE TABLE public."global_file_collection_config"(
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    auto_faiss_sync BOOLEAN NOT NULL DEFAULT FALSE
);