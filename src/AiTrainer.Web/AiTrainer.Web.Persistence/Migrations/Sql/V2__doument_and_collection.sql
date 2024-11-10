CREATE TABLE public."file_collection" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES public."user"(id),
    name TEXT NOT NULL,
    parent_id UUID, 
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    date_modified TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    FOREIGN KEY (parent_id) REFERENCES public."file_collection"(id) ON DELETE SET NULL,
    CONSTRAINT unique_name_with_parent UNIQUE (parent_id, name)
);

CREATE UNIQUE INDEX unique_name_with_null_parent
ON public."file_collection" (name)
WHERE parent_id IS NULL;

CREATE TABLE public."file_document" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    collection_id UUID NOT NULL REFERENCES public."file_collection"(id),
    file_type INTEGER NOT NULL,
    file_name TEXT NOT NULL,
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    CONSTRAINT unique_collection_file_name UNIQUE (collection_id, file_name)
);
