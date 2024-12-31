import { FlatFileDocumentPartialCollection } from "../../Models/FlatFileDocumentPartialCollection";
import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
} from "@mui/material";
import { NewFileTableCollectionRow } from "./NewFileTableCollectionRow";
import { NewFileTableDocumentRow } from "./NewFileTableDocumentRow";
import { useFileCollectionContextMenuContext } from "../Contexts/FileCollectionContextMenuContext";

export const NewFileTable: React.FC<{
  flatCollection?: FlatFileDocumentPartialCollection | null;
}> = ({ flatCollection }) => {
  const { menuPosition, handleRightClick, closeContextMenu } =
    useFileCollectionContextMenuContext();
  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableCell
            sx={{
              maxWidth: "200px", // Prevent excessive growth
              whiteSpace: "nowrap", // Prevent wrapping
              overflow: "hidden",
              textOverflow: "ellipsis", // Add ellipsis if text overflows
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
        </TableHead>
        <TableBody>
          {flatCollection?.fileCollections.map((fileCollection) => (
            <NewFileTableCollectionRow
              closeContextMenu={closeContextMenu}
              handleRightClick={handleRightClick}
              menuPosition={menuPosition}
              fileCollection={fileCollection}
              key={fileCollection.id}
            />
          ))}
          {flatCollection?.fileDocuments.map((fileDoc) => (
            <NewFileTableDocumentRow fileDocPartial={fileDoc} />
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
