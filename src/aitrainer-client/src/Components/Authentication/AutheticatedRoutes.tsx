import { CollectionHome } from "../Pages/CollectionHome";

export const AuthenticatedRoutes: {
  link: string;
  component: () => JSX.Element;
}[] = [
  {
    component: () => <CollectionHome />,
    link: "/collection/home/:id",
  },
  {
    component: () => <CollectionHome />,
    link: "/collection/home",
  },
];
