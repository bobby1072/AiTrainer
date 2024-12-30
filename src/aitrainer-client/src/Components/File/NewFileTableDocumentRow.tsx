import {
  Box,
  IconButton,
  TableCell,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import { FileDocumentPartial } from "../../Models/FileDocument";
import { prettyDateWithTime } from "../../Utils/DateTime";
import DeleteIcon from "@mui/icons-material/Delete";
import { useDeleteFileDocumentMutation } from "../../Hooks/useDeleteFileDocumentMutation";
import { useEffect } from "react";
import { useSnackbar } from "notistack";
import { getFileExtension } from "../../Utils/FileUtils";
const fileDoc = require("./fileDoc.png");

export const NewFileTableDocumentRow: React.FC<{
  fileDocPartial: FileDocumentPartial;
}> = ({ fileDocPartial }) => {
  const dateCreated = new Date(fileDocPartial.dateCreated);
  const { mutate, isLoading, data } = useDeleteFileDocumentMutation();
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    if (data) {
      enqueueSnackbar(`Document deleted`, { variant: "error" });
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
              gap: 3.5,
            }}
          >
            <Box
              component="img"
              sx={{
                width: "2.5%",
              }}
              src={fileDoc}
              alt={`fileDocImage: ${fileDocPartial.id}`}
            />
            <Typography>
              {fileDocPartial.fileName}
              {getFileExtension(fileDocPartial.fileType)}
            </Typography>
          </Box>
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`${dateCreated.toISOString()}`}>
            <>{prettyDateWithTime(dateCreated)}</>
          </Tooltip>
        </TableCell>
        <TableCell align="right" />
        <TableCell align="right">
          <IconButton
            color="inherit"
            size="small"
            disabled={isLoading}
            onClick={() => {
              mutate({ id: fileDocPartial.id! });
            }}
          >
            <DeleteIcon />
          </IconButton>
        </TableCell>
      </TableRow>
    </>
  );
};
