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
import DownloadIcon from "@mui/icons-material/Download";
import { useDownloadFileDocumentMutation } from "../../Hooks/useDownloadFileDocumentMutation";
import InfoIcon from "@mui/icons-material/Info";

const fileDoc = require("./fileDoc.png");

export const NewFileTableDocumentRow: React.FC<{
  fileDocPartial: FileDocumentPartial;
}> = ({ fileDocPartial }) => {
  const {
    mutate: deleteFile,
    isLoading: isDeleteFileLoading,
    data: deleteFileData,
  } = useDeleteFileDocumentMutation();
  const { mutate: downloadFile, isLoading: isDownloadFileLoading } =
    useDownloadFileDocumentMutation();
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    if (deleteFileData) {
      enqueueSnackbar(`Document deleted`, { variant: "error" });
    }
  }, [deleteFileData, enqueueSnackbar]);

  const handleDownload = () => {
    downloadFile(
      { fileDocId: fileDocPartial.id! },
      {
        onSuccess: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement("a");
          link.href = url;

          const fileName = `${fileDocPartial.fileName || "downloaded_file"}${
            getFileExtension(fileDocPartial.fileType) || ""
          }`;

          link.setAttribute("download", fileName);
          document.body.appendChild(link);
          link.click();

          link.parentNode?.removeChild(link);
          window.URL.revokeObjectURL(url);
        },
        onError: () => {
          enqueueSnackbar("Failed to download the file.", { variant: "error" });
        },
      }
    );
  };

  const dateCreated = new Date(fileDocPartial.dateCreated);
  return (
    <>
      <TableRow>
        <TableCell
          sx={{
            maxWidth: "200px",
            wordWrap: "break-word",
            whiteSpace: "normal",
            textOverflow: "ellipsis",
          }}
        >
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
                width: "9%",
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
        <TableCell
          align="left"
          sx={{
            wordWrap: "break-word",
            whiteSpace: "normal",
            maxWidth: "500px",
          }}
        >
          {fileDocPartial.fileDescription}
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`${dateCreated.toISOString()}`}>
            <Typography>{prettyDateWithTime(dateCreated)}</Typography>
          </Tooltip>
        </TableCell>
        <TableCell align="right" />
        {fileDocPartial.metaData ? (
          <TableCell align="right">
            <IconButton color="inherit" size="small">
              <InfoIcon />
            </IconButton>
          </TableCell>
        ) : (
          <TableCell align="right" />
        )}
        <TableCell align="right">
          <IconButton
            color="inherit"
            size="small"
            disabled={isDownloadFileLoading}
            onClick={handleDownload}
          >
            <DownloadIcon />
          </IconButton>
        </TableCell>
        <TableCell align="right">
          <IconButton
            color="inherit"
            size="small"
            disabled={isDeleteFileLoading}
            onClick={() => {
              deleteFile({ id: fileDocPartial.id! });
            }}
          >
            <DeleteIcon />
          </IconButton>
        </TableCell>
      </TableRow>
    </>
  );
};
