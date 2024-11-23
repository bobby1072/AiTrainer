import { useGetClientConfigurationQuery } from "../../Hooks/useGetClientConfigurationQuery";
import { Loading } from "../Common/Loading";
import { PageBase } from "../Common/PageBase";
export const LoginPage: React.FC = () => {
  const { isLoading } = useGetClientConfigurationQuery();

  if (isLoading) return <Loading fullScreen />;
  return (
    <PageBase>
      <h1>Hello world</h1>
    </PageBase>
  );
};
