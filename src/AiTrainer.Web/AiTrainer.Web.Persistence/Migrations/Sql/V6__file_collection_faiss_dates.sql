ALTER TABLE public."file_collection_faiss" ADD COLUMN date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now();
ALTER TABLE public."file_collection_faiss" ADD COLUMN date_modified TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now();
