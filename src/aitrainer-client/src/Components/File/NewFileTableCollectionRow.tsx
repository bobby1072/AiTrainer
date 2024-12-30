import React, { useEffect } from "react";
import { useDeleteFileCollectionMutation } from "../../Hooks/useDeleteFileCollectionMutation";
import { FileCollection } from "../../Models/FileCollection";
import {
  Box,
  ButtonBase,
  IconButton,
  TableCell,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import { prettyDateWithTime } from "../../Utils/DateTime";
import { useSnackbar } from "notistack";

const fileCol = require("./fileCol.png");

export const NewFileTableCollectionRow: React.FC<{
  fileCollection: FileCollection;
}> = ({ fileCollection }) => {
  const dateCreated = new Date(fileCollection.dateCreated);
  const dateModified = new Date(fileCollection.dateModified);
  const { mutate, isLoading, data } = useDeleteFileCollectionMutation();
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    if (data) {
      enqueueSnackbar(`Collection deleted`, { variant: "error" });
    }
  }, [data, enqueueSnackbar]);

  return (
    <>
      <TableRow>
        <TableCell>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 3,
            }}
          >
            <Box
              component="img"
              sx={{
                width: "3%",
              }}
              src={fileCol}
              alt={`fileColImage: ${fileCollection.id}`}
            />
            <ButtonBase
              href={`/collection/home/${fileCollection.id}`}
              sx={{
                "&:hover": {
                  textDecoration: "underline",
                },
              }}
            >
              <Typography>{fileCollection.collectionName}</Typography>
            </ButtonBase>
          </Box>
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`${dateCreated.toISOString()}`}>
            <>{prettyDateWithTime(dateCreated)}</>
          </Tooltip>
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`${dateModified.toISOString()}`}>
            <>{prettyDateWithTime(dateModified)}</>
          </Tooltip>
        </TableCell>
        <TableCell align="right">
          <IconButton
            color="inherit"
            size="small"
            disabled={isLoading}
            onClick={() => {
              mutate({ id: fileCollection.id! });
            }}
          >
            <DeleteIcon />
          </IconButton>
        </TableCell>
      </TableRow>
    </>
  );
};
