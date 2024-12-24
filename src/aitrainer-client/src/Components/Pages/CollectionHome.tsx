import { Grid2 } from "@mui/material";
import { PageBase } from "../Common/PageBase";
import { useGetTopLayerOfFileQuery } from "../../Hooks/useGetTopLayerOfFileQuery";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";
import { useParams } from "react-router-dom";
import { CollectionDocumentTable } from "../File/CollectionDocumentTable";

export const CollectionHome: React.FC = () => {
  const { id: groupId } = useParams<{ id?: string }>();
  const { data, error, isLoading } = useGetTopLayerOfFileQuery(groupId);

  if (isLoading) return <Loading fullScreen />;
  else if (error)
    return <ErrorComponent fullScreen errorMessage={error.message} />;
  else if (!data) return <ErrorComponent fullScreen />;
  return (
    <PageBase>
      <Grid2
        container
        height={"100vh"}
        justifyContent="center"
        alignItems="center"
        direction="column"
        width="100%"
      >
        <Grid2 width={"100%"}>
          <CollectionDocumentTable flatCollection={data} />
        </Grid2>
      </Grid2>
    </PageBase>
  );
};
