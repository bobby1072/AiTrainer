import { TestHome } from "../Pages/TestHome";

export const AuthenticatedRoutes: {
  link: string;
  component: () => JSX.Element;
}[] = [
  {
    component: () => <TestHome />,
    link: "/TestHome",
  },
];
