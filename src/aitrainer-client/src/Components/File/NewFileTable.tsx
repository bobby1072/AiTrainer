import React from "react";
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

export const NewFileTable: React.FC<{
  flatCollection?: FlatFileDocumentPartialCollection | null;
}> = ({ flatCollection }) => {
  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableCell>
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
                  width: "3%",
                }}
              />
              Name
            </Box>
          </TableCell>
          <TableCell align="right">Date Created</TableCell>
          <TableCell align="right">Date Modified</TableCell>
          <TableCell align="right" />
        </TableHead>
        <TableBody>
          {flatCollection?.fileCollections.map((fileCollection) => (
            <NewFileTableCollectionRow
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
