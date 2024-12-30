import { DialogTitle, styled, Typography } from "@mui/material";

const StyledStyledDialogTitle = styled(DialogTitle)(({ theme }) => ({
  color: "white",
  backgroundColor: theme.palette.primary.main,
  textAlign: "center",
}));

export const StyledDialogTitle: React.FC<{ title: string }> = ({ title }) => {
  return (
    <StyledStyledDialogTitle>
      <Typography variant="h6">{title}</Typography>
    </StyledStyledDialogTitle>
  );
};
