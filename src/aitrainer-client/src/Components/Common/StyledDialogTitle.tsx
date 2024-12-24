import { DialogTitle, styled } from "@mui/material";

export const StyledDialogTitle = styled(DialogTitle)(({ theme }) => ({
  color: "white",
  backgroundColor: theme.palette.primary.main,
  textAlign: "center",
}));
