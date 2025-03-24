import React, { useState, useEffect } from "react";

export const usePersistedState = <T>(
  keyName: string,
  defaultValue?: T
): [T, React.Dispatch<React.SetStateAction<T>>] => {
  const [value, setValue] = useState<T>(() => {
    const stickyValue = window.localStorage.getItem(keyName);
    return stickyValue !== null && stickyValue !== undefined
      ? JSON.parse(stickyValue)
      : defaultValue;
  });

  useEffect(() => {
    if (value === undefined) window.localStorage.removeItem(keyName);
    else window.localStorage.setItem(keyName, JSON.stringify(value));
  });

  return [value, setValue];
};
