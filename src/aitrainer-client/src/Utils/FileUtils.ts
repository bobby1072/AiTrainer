export const getFileExtension = (fileTypeEnumNum: number): string => {
  switch (fileTypeEnumNum) {
    case 1:
      return ".txt";
    case 2:
      return ".pdf";
    default:
      throw new Error("Invalid file type");
  }
};
