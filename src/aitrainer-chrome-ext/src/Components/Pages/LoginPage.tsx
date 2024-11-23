import { useGetClientConfigurationQuery } from "../../Hooks/useGetClientConfigurationQuery";
import { Loading } from "../Common/Loading";
import "./../../App.css";
export const LoginPage: React.FC = () => {
  const { isLoading } = useGetClientConfigurationQuery();

  if (isLoading) return <Loading fullScreen />;
  return (
    <div className="App" style={{ minWidth: "30vh", minHeight: "40vh" }}>
      <h1>Hello world</h1>
    </div>
  );
};
