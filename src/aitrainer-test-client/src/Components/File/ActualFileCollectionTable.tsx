import { FlatFileDocumentPartialCollection } from "../../Models/FlatFileDocumentPartialCollection";
import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
} from "@mui/material";
import { ActualFileCollectionTableDocumentRow } from "./ActualFileCollectionTableDocumentRow";
import { useFileCollectionContextMenuContext } from "../Contexts/FileCollectionContextMenuContext";
import { ActualFileCollectionTableCollectionRow } from "./ActualFileCollectionTableCollectionRow";

export const ActualFileCollectionTable: React.FC<{
  flatCollection?: FlatFileDocumentPartialCollection | null;
  singleLevel: boolean;
}> = ({ flatCollection, singleLevel }) => {
  const { handleRightClick, closeContextMenu } =
    useFileCollectionContextMenuContext();
  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableCell
            sx={{
              maxWidth: "200px",
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
          >
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 3,
              }}
            >
              <Box
                component="span"
                sx={{
                  width: "10%",
                }}
              />
              Name
            </Box>
          </TableCell>
          <TableCell align="left">Description</TableCell>
          <TableCell align="right">Date Created</TableCell>
          <TableCell align="right">Date Modified</TableCell>
          <TableCell align="right" />
          <TableCell align="right" />
          <TableCell align="right" />
        </TableHead>
        <TableBody>
          {flatCollection?.fileCollections.map((fileCollection) => (
            <ActualFileCollectionTableCollectionRow
              closeContextMenu={closeContextMenu}
              handleRightClick={handleRightClick}
              fileCollection={fileCollection}
              key={fileCollection.id}
              singleLevel={singleLevel}
            />
          ))}
          {flatCollection?.fileDocuments.map((fileDoc) => (
            <ActualFileCollectionTableDocumentRow fileDocPartial={fileDoc} />
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
