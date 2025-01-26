export type DocStore = [
  [
    [
      string,
      {
        pageContent: string;
        metadata: {
          source: string;
        };
      }
    ],
    {
      [key: string]: string;
    }
  ]
];
