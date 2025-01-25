CREATE TABLE public."file_document_metadata"(
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    document_id UUID NOT NULL REFERENCES public."file_document"(id) ON DELETE CASCADE ON UPDATE CASCADE UNIQUE UNIQUE,
    Title TEXT,
    Author TEXT,
    Subject TEXT,
    Keywords TEXT,
    Creator TEXT,
    Producer TEXT,
    CreatedDate TEXT,
    ModifiedDate TEXT,
    NumberOfPages INTEGER,
    IsEncrypted BOOLEAN DEFAULT FALSE,
    ExtraData JSONB
);