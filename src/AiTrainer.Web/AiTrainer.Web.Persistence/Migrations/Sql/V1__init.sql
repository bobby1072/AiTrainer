CREATE TABLE public."user" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    name TEXT,
    username TEXT NOT NULL UNIQUE,
    date_created TIMESTAMP NOT NULL DEFAULT now(),
    date_modified TIMESTAMP NOT NULL DEFAULT now()
);