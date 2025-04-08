CREATE TABLE public."single_document_chunk"(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    page_content TEXT NOT NULL,
    file_document_id UUID NOT NULL REFERENCES public."file_document"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    metadata JSONB NOT NULL
);