import { styled } from "@mui/material";

const StyledPageDiv = styled("div")(() => ({
  height: "20rem",
  width: "30rem",
  display: "flex",
  backgroundColor: "#F0F0F0",
}));

export const PageBase: React.FC<{ children?: React.ReactNode }> = ({
  children,
}) => {
  return <StyledPageDiv>{children}</StyledPageDiv>;
};
