export type DocStorePageInfo = {
  pageContent?: string | null;
  metadata: {
    source?: string | null;
  };
};

export type DocStore = [[string, DocStorePageInfo][], Record<string, string>];
