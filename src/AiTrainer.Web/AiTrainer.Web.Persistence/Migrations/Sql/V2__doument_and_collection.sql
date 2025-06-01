CREATE TABLE public."file_collection" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    collection_name TEXT NOT NULL,
    collection_description TEXT,
    parent_id UUID REFERENCES public."file_collection"(id) 
        ON DELETE CASCADE     
        ON UPDATE CASCADE, 
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    date_modified TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),    
    CONSTRAINT unique_name_with_parent UNIQUE (parent_id, collection_name)
);

CREATE UNIQUE INDEX unique_name_with_null_parent
ON public."file_collection" (collection_name, user_id)
WHERE parent_id IS NULL;

CREATE TABLE public."file_collection_faiss"(
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    collection_id UUID REFERENCES public."file_collection"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    faiss_index BYTEA NOT NULL,
    faiss_json JSONB NOT NULL,
    CONSTRAINT unique_user_with_collection_id UNIQUE (user_id, collection_id) 
);

CREATE TABLE public."file_document" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    collection_id UUID REFERENCES public."file_collection"(id) 
        ON DELETE CASCADE    
        ON UPDATE CASCADE,    
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    file_type INTEGER NOT NULL,
    file_name TEXT NOT NULL,
    file_description TEXT,
    file_data BYTEA NOT NULL,
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    CONSTRAINT unique_collection_file_name_type UNIQUE (collection_id, file_name, file_type)
);

CREATE UNIQUE INDEX unique_name_with_null_collection
ON public."file_document" (file_name, user_id)
WHERE collection_id IS NULL;
