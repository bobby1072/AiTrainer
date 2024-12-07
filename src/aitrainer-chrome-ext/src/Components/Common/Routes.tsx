import { TestHome } from "../Pages/TestHome";

export const Routes: {
  link: string;
  component: () => JSX.Element;
}[] = [
  {
    component: () => <TestHome />,
    link: "/TestHome",
  },
];
