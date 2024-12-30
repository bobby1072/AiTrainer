export const getFileExtension = (fileTypeEnumNum: number): string => {
  switch (fileTypeEnumNum) {
    case 0:
      return ".pdf";
    case 1:
      return ".txt";
    default:
      throw new Error("Invalid file type");
  }
};
