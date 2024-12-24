import { TestHome } from "../Pages/TestHome";

export const AuthenticatedRoutes: {
  link: string;
  component: () => JSX.Element;
}[] = [
  {
    component: () => <TestHome />,
    link: "/collection/home/:id",
  },
  {
    component: () => <TestHome />,
    link: "/collection/home",
  },
];
