CREATE TABLE public."file_document_metadata"(
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    document_id UUID NOT NULL REFERENCES public."file_document"(id) ON DELETE CASCADE ON UPDATE CASCADE UNIQUE,
    title TEXT,
    author TEXT,
    subject TEXT,
    keywords TEXT,
    creator TEXT,
    producer TEXT,
    created_date TEXT,
    modified_date TEXT,
    number_of_pages INTEGER,
    is_encrypted BOOLEAN DEFAULT FALSE,
    extra_data JSONB
);