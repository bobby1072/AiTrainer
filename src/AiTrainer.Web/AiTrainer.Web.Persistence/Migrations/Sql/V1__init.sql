CREATE TABLE public.world_fish (
    scientific_name TEXT,
    taxocode TEXT PRIMARY KEY,
    isscaap TEXT,
    a3_code TEXT,
    english_name TEXT,
    nickname TEXT
);

CREATE TABLE public."user" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    name TEXT,
    email_verified BOOLEAN NOT NULL DEFAULT FALSE,
    username TEXT NOT NULL UNIQUE
);

CREATE TABLE public."group" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT,
    leader_id UUID NOT NULL,
    catches_public BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    public BOOLEAN NOT NULL DEFAULT FALSE,
    listed BOOLEAN NOT NULL DEFAULT FALSE,
    emblem BYTEA,
    CONSTRAINT group_leader_id_fk FOREIGN KEY (leader_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.group_position (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    group_id UUID NOT NULL,
    name TEXT NOT NULL,
    can_manage_group BOOLEAN NOT NULL DEFAULT FALSE,
    can_read_catches BOOLEAN NOT NULL DEFAULT TRUE,
    can_manage_catches BOOLEAN NOT NULL DEFAULT FALSE,
    can_read_members BOOLEAN NOT NULL DEFAULT TRUE,
    can_manage_members BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT group_position_unique UNIQUE (group_id, name),
    CONSTRAINT group_position_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.group_member (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    group_id UUID NOT NULL,
    user_id UUID NOT NULL,
    position_id INTEGER NOT NULL,
    CONSTRAINT group_member_unique UNIQUE (group_id, user_id),
    CONSTRAINT group_member_position_id_fk FOREIGN KEY (position_id) REFERENCES public.group_position(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT group_member_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT group_member_user_id_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.group_catch (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    group_id UUID NOT NULL,
    user_id UUID NOT NULL,
    species TEXT NOT NULL,
    weight DECIMAL,
    length DECIMAL,
    description TEXT,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    caught_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    catch_photo BYTEA,
    latitude DECIMAL NOT NULL,
    longitude DECIMAL NOT NULL,
    world_fish_taxocode TEXT,
    CONSTRAINT group_catch_world_fish_taxocode_fk FOREIGN KEY (world_fish_taxocode) REFERENCES public.world_fish(taxocode) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT group_catch_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT group_catch_user_id_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.group_catch_comment (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    catch_id UUID NOT NULL,
    user_id UUID NOT NULL,
    comment TEXT NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT group_catch_id_group_catch_comment_fk FOREIGN KEY (catch_id) REFERENCES public.group_catch(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT user_id_group_catch_comment_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.group_catch_comment_tagged_users (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    comment_id INTEGER NOT NULL,
    user_id UUID NOT NULL,
    CONSTRAINT group_catch_comment_tagged_users_group_catch_comment_id_fk FOREIGN KEY (comment_id) REFERENCES public.group_catch_comment(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT group_catch_comment_tagged_users_user_id_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT group_catch_comment_tagged_users_unique UNIQUE (comment_id, user_id)
);
