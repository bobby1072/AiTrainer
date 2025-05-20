CREATE TABLE public."shared_file_collection_member"(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    collection_id UUID REFERENCES public."file_collection"(id) 
        ON DELETE CASCADE    
        ON UPDATE CASCADE,
    can_view_documents BOOLEAN NOT NULL DEFAULT FALSE,    
    can_download_documents BOOLEAN NOT NULL DEFAULT FALSE,    
    can_create_documents BOOLEAN NOT NULL DEFAULT FALSE,
    can_remove_documents BOOLEAN NOT NULL DEFAULT FALSE
);