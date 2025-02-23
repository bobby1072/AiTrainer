import { Grid2, Paper } from "@mui/material";
import { PageBase } from "../Common/PageBase";
import { useGetTopLayerOfFileQuery } from "../../Hooks/useGetTopLayerOfFileQuery";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";
import { useParams } from "react-router-dom";
import { CollectionDocumentTable } from "../File/CollectionDocumentTable";
import { FileCollectionContextMenuContextProvider } from "../Contexts/FileCollectionContextMenuContext";
import { useEffect, useState } from "react";
import { CollectionFaissSearch } from "../File/CollectionFaissSearch";

export const CollectionHome: React.FC = () => {
  const { id: collectionId } = useParams<{ id?: string }>();
  const [searchMode, setSearchMode] = useState<boolean>(false);
  const { data, error, isLoading, refetch } =
    useGetTopLayerOfFileQuery(collectionId);
  useEffect(() => {
    refetch();
  }, [collectionId, refetch]);
  if (isLoading) return <Loading fullScreen />;
  else if (error)
    return <ErrorComponent fullScreen errorMessage={error.message} />;
  else if (!data) return <ErrorComponent fullScreen />;
  return (
    <PageBase>
      <FileCollectionContextMenuContextProvider>
        <Grid2
          container
          height={"100vh"}
          justifyContent="center"
          alignItems="center"
          direction="column"
          width="100%"
        >
          {searchMode ? (
            <Grid2 width={"90%"}>
              <CollectionFaissSearch collectionId={collectionId!} />
            </Grid2>
          ) : (
            <Grid2 width={"90%"}>
              <Paper
                elevation={1}
                sx={{
                  border: "1px solid #ccc",
                  borderRadius: "8px",
                  overflow: "hidden",
                }}
              >
                <CollectionDocumentTable
                  flatCollection={data}
                  changeMode={() => setSearchMode(true)}
                />
              </Paper>
            </Grid2>
          )}
        </Grid2>
      </FileCollectionContextMenuContextProvider>
    </PageBase>
  );
};
