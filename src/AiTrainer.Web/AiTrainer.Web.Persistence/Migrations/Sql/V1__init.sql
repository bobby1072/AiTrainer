CREATE TABLE public."solicited_device_token"(
    device_token UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    in_use BOOLEAN NOT NULL DEFAULT FALSE,
    solicited_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    expires_at TIMESTAMP WITHOUT TIME ZONE NOT NULL
);

CREATE TABLE public."user" (
    id UUID PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    name TEXT,
    date_created TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    date_modified TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    CONSTRAINT fk_device_token FOREIGN KEY (id) REFERENCES public."solicited_device_token"(device_token)  ON DELETE CASCADE     
        ON UPDATE CASCADE
);
