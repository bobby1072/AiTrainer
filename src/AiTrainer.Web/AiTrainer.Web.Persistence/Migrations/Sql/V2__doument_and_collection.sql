CREATE TABLE public."file_collection" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES public."user"(id),
    name TEXT NOT NULL,
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    date_modified TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now()
);


CREATE TABLE public."file_collection_nest"(
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    parent_file_collection_id UUID NOT NULL REFERENCES public."file_collection"(id),
    child_file_collection_id UUID NOT NULL REFERENCES public."file_collection"(id),
    CONSTRAINT parent_child_different CHECK (parent_file_collection_id <> child_file_collection_id)
);


CREATE TABLE public."file_document" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    collection_id UUID NOT NULL REFERENCES public."file_collection"(id),
    file_type INTEGER NOT NULL,
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now()
);