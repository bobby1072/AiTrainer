// import { differenceInDays } from "date-fns";

export const INCLUSIVE_DATE: boolean = true;
export const EXCLUSIVE_DATE: boolean = false;

export interface TimePeriod {
  id: string;
  startDate: Date;
  endDate: Date;
  totalDays: number;
}
export const getEarliestAndLatestDate = (dateList: Date[]) => {
  const { earliestDate, latestDate } = dateList.reduce<{
    earliestDate?: Date;
    latestDate?: Date;
  }>(
    (a, b) => ({
      earliestDate:
        !a.earliestDate || a.earliestDate.getTime() > b.getTime()
          ? b
          : a.earliestDate,
      latestDate:
        !a.latestDate || a.latestDate.getTime() < b.getTime()
          ? b
          : a.latestDate,
    }),
    {}
  );
  return { earliestDate, latestDate };
};
const parseAndValidateDate = (
  date: Date | string,
  argumentName: string
): Date => {
  const copy = new Date(date);
  if (isNaN(copy.getTime()))
    throw new Error(`${argumentName} is not a valid date`);

  return copy;
};

export const prettyDateWithTime = (date: Date): string => {
  const dateMinutes = date.getUTCMinutes();
  return `${prettyDateWithYear(date)} ${date.getUTCHours()}:${
    dateMinutes.toString().length < 2 ? `0${dateMinutes}` : dateMinutes
  }`;
};

export const prettyDateCurrentYearWithTime = (date: Date): string => {
  const d = new Date(date);
  const mo = new Intl.DateTimeFormat(navigator.language, {
    month: "short",
  }).format(d);
  const da = new Intl.DateTimeFormat(navigator.language, {
    day: "2-digit",
  }).format(d);
  const hr = new Intl.DateTimeFormat(navigator.language, {
    hour: "2-digit",
    hour12: false,
  }).format(d);
  const min = new Intl.DateTimeFormat(navigator.language, {
    minute: "numeric",
  }).format(d);
  return `${da}-${mo} ${hr}:${min}`;
};

export const prettyDateCurrentYear = (date: Date): string => {
  const d = new Date(date);
  const mo = new Intl.DateTimeFormat(navigator.language, {
    month: "short",
  }).format(d);
  const da = new Intl.DateTimeFormat(navigator.language, {
    day: "2-digit",
  }).format(d);
  return `${da}-${mo}`;
};

export const ariaDayInWeekCurrentYear = (date: Date | string): string => {
  const d = parseAndValidateDate(date, "Date");
  const weekDay = new Intl.DateTimeFormat(navigator.language, {
    weekday: "long",
  }).format(d);
  const mo = new Intl.DateTimeFormat(navigator.language, {
    month: "short",
  }).format(d);
  const da = new Intl.DateTimeFormat(navigator.language, {
    day: "2-digit",
  }).format(d);
  return `${weekDay} ${da} ${mo}`;
};

export const prettyDateWithYear = (date: Date): string => {
  const d = new Date(date);
  const ye = new Intl.DateTimeFormat(navigator.language, {
    year: "numeric",
  }).format(d);
  const mo = new Intl.DateTimeFormat(navigator.language, {
    month: "short",
  }).format(d);
  const da = new Intl.DateTimeFormat(navigator.language, {
    day: "2-digit",
  }).format(d);
  return `${da}-${mo}-${ye}`;
};

export const dateFormatterEquivalent = (date?: Date): string | undefined => {
  if (!date) return undefined;
  const d = new Date(date);
  const ye = new Intl.DateTimeFormat(navigator.language, {
    year: "numeric",
  }).format(d);
  const mo = new Intl.DateTimeFormat(navigator.language, {
    month: "short",
  }).format(d);
  const da = new Intl.DateTimeFormat(navigator.language, {
    day: "numeric",
  }).format(d);
  return `${mo} ${da}, ${ye}`;
};

export const prettyMonthWithYear = (date: Date): string => {
  const d = new Date(date);
  const ye = new Intl.DateTimeFormat(navigator.language, {
    year: "numeric",
  }).format(d);
  const mo = new Intl.DateTimeFormat(navigator.language, {
    month: "short",
  }).format(d);
  return `${mo}-${ye}`;
};

// export const dayDiff = (date1: Date, date2: Date): number => differenceInDays(date2, date1);

export const isAfter = (date1: Date, date2: Date): boolean => {
  return date2.getTime() > date1.getTime();
};

export const isDateAfter = (
  comparisonDate: Date | string,
  refDate: Date | string
): boolean => {
  const comparisonDateCopy = parseAndValidateDate(
    comparisonDate,
    "Comparison date"
  );
  const refDateCopy = parseAndValidateDate(refDate, "Ref date");

  comparisonDateCopy.setHours(0, 0, 0, 0);
  refDateCopy.setHours(0, 0, 0, 0);

  return comparisonDateCopy.getTime() > refDateCopy.getTime();
};

export const isDateAfterOrEqual = (
  comparisonDate: Date | string,
  refDate: Date | string
): boolean => {
  const comparisonDateCopy = parseAndValidateDate(
    comparisonDate,
    "Comparison date"
  );
  const refDateCopy = parseAndValidateDate(refDate, "Ref date");

  comparisonDateCopy.setHours(0, 0, 0, 0);
  refDateCopy.setHours(0, 0, 0, 0);

  return comparisonDateCopy.getTime() >= refDateCopy.getTime();
};

export const isDateBefore = (
  comparisonDate: Date | string,
  refDate: Date | string
): boolean => {
  const comparisonDateCopy = parseAndValidateDate(
    comparisonDate,
    "Comparison date"
  );
  const refDateCopy = parseAndValidateDate(refDate, "Ref date");

  comparisonDateCopy.setHours(0, 0, 0, 0);
  refDateCopy.setHours(0, 0, 0, 0);

  return comparisonDateCopy.getTime() < refDateCopy.getTime();
};

export const isDateBeforeOrEqual = (
  comparisonDate: Date | string,
  refDate: Date | string
): boolean => {
  const comparisonDateCopy = parseAndValidateDate(
    comparisonDate,
    "Comparison date"
  );
  const refDateCopy = parseAndValidateDate(refDate, "Ref date");

  comparisonDateCopy.setHours(0, 0, 0, 0);
  refDateCopy.setHours(0, 0, 0, 0);

  return comparisonDateCopy.getTime() <= refDateCopy.getTime();
};

export const isDateEqual = (
  comparisonDate: Date | string,
  refDate: Date | string
): boolean => {
  const comparisonDateCopy = parseAndValidateDate(
    comparisonDate,
    "Comparison date"
  );
  const refDateCopy = parseAndValidateDate(refDate, "Ref date");

  comparisonDateCopy.setHours(0, 0, 0, 0);
  refDateCopy.setHours(0, 0, 0, 0);

  return comparisonDateCopy.getTime() === refDateCopy.getTime();
};

export const ocurredInRange = (
  valDate?: Date | string | null,
  startRange?: Date,
  incStart?: boolean,
  endRange?: Date,
  incEnd?: boolean
): boolean => {
  if (valDate) {
    const inclusiveStart = incStart ? incStart : EXCLUSIVE_DATE;
    const inclusiveEnd = incEnd ? incEnd : EXCLUSIVE_DATE;
    const valDateAsDate = parseAndValidateDate(valDate, "Date");
    if (startRange && endRange) {
      const start = inclusiveStart
        ? isDateAfterOrEqual(valDateAsDate, startRange)
        : isDateAfter(valDateAsDate, startRange);
      const end = inclusiveEnd
        ? isDateBeforeOrEqual(valDateAsDate, endRange)
        : isDateBefore(valDateAsDate, endRange);
      return start && end;
    }
    if (startRange) {
      return inclusiveStart
        ? isDateAfterOrEqual(valDateAsDate, startRange)
        : isDateAfter(valDateAsDate, startRange);
    }
    if (endRange) {
      return inclusiveEnd
        ? isDateBeforeOrEqual(valDateAsDate, endRange)
        : isDateBefore(valDateAsDate, endRange);
    }
  }
  return false;
};

export const ocurredBefore = (
  valDate?: Date | string | null,
  endRange?: Date,
  incEnd?: boolean
): boolean => {
  return ocurredInRange(valDate, undefined, false, endRange, incEnd);
};

export const ocurredAfter = (
  valDate?: Date | string | null,
  startRange?: Date,
  incStart?: boolean
): boolean => {
  return ocurredInRange(valDate, startRange, incStart, undefined, false);
};

export const getMondayThisWeek = (dateInMonth?: Date): Date => {
  const referenceDate = dateInMonth ? new Date(dateInMonth) : new Date();
  referenceDate.setUTCHours(0);
  referenceDate.setUTCMinutes(0);
  referenceDate.setUTCSeconds(0);
  referenceDate.setUTCMilliseconds(0);
  // Make Monday 0
  const currentDayOfWeek =
    referenceDate.getUTCDay() === 0 ? 6 : referenceDate.getUTCDay() - 1;
  // Reset to Monday
  return addDaysToDate(referenceDate, -1 * currentDayOfWeek);
};

export const get1stOfMonth = (dateInMonth: Date = new Date()): Date => {
  const dateInMonthCopy = new Date(dateInMonth);
  dateInMonthCopy.setHours(0, 0, 0, 0);
  return new Date(dateInMonthCopy.getFullYear(), dateInMonthCopy.getMonth(), 1);
};

export const getLastDayOfMonth = (dateInMonth: Date = new Date()): Date => {
  const lastDayOfMonth = new Date(
    dateInMonth.getFullYear(),
    dateInMonth.getMonth() + 1,
    0
  );
  lastDayOfMonth.setHours(0, 0, 0, 0);

  return lastDayOfMonth;
};

export const getMondayOfFirstPartialWeekOfMonth = (
  dateInMonth?: Date
): Date => {
  const referenceDate = dateInMonth ? dateInMonth : new Date();
  const monthStart = get1stOfMonth(referenceDate);
  const monthStartDay = monthStart.getUTCDay();
  // If first day of month is a Saturday or Sunday (6, 0) then following Monday otherwise preceeding Monday
  if (monthStartDay === 6 || monthStartDay === 0) {
    return getMondayThisWeek(addDaysToDate(monthStart, 7));
  } else {
    return getMondayThisWeek(monthStart);
  }
};

export const getMondayOfFirstFullWeekOfMonth = (dateInMonth?: Date): Date => {
  const referenceDate = dateInMonth ? dateInMonth : new Date();
  const monthStart = get1stOfMonth(referenceDate);
  const monthStartDay = monthStart.getUTCDay();
  // If first day of month is a Monday then that Monday, otherwise following Monday
  if (monthStartDay === 1) {
    return monthStart;
  } else {
    return getMondayThisWeek(addDaysToDate(monthStart, 7));
  }
};

export const getFridayOfLastPartialWeekOfMonth = (dateInMonth?: Date): Date => {
  const referenceDate = dateInMonth ? dateInMonth : new Date();
  const monthEnd = getLastDayOfMonth(referenceDate);
  return addDaysToDate(getMondayThisWeek(monthEnd), 4);
};

export const addDaysToDate = (date: Date, days: number): Date => {
  const dateCopy = new Date(date);
  dateCopy.setDate(dateCopy.getDate() + days);

  return dateCopy;
};

export const addMonthsToDate = (date: Date, months: number): Date => {
  const dateCopy = new Date(date);
  dateCopy.setDate(1);
  dateCopy.setMonth(dateCopy.getMonth() + months);

  return dateCopy;
};

export const getDaysOfWeek = (mondayDate: Date): Date[] => {
  const monday: Date = mondayDate;
  const tuesday: Date = addDaysToDate(monday, 1);
  const wednesday: Date = addDaysToDate(monday, 2);
  const thursday: Date = addDaysToDate(monday, 3);
  const friday: Date = addDaysToDate(monday, 4);
  return [monday, tuesday, wednesday, thursday, friday];
};

// export const isContiguous = (
//   isNonWorkingDay: (date: Date) => boolean,
//   date1End?: Date,
//   date2Start?: Date
// ): boolean => {
//   if (!date1End || !date2Start) {
//     return false;
//   }
//   // check the dates are in the expected order
//   if (!isAfter(date1End, date2Start)) {
//     return false;
//   }
//   const diffDays = dayDiff(
//     new Date(date1End.toDateString()),
//     new Date(date2Start.toDateString())
//   );
//   // Check for a half-day separation
//   if (
//     diffDays === 1 &&
//     date1End.getUTCHours() > 15 &&
//     date2Start.getUTCHours() > 11
//   ) {
//     return false;
//   }
//   // Simplest scenario consecutive days
//   if (diffDays === 1) {
//     return true;
//   }
//   // Standard weekend or bank holiday period
//   if (diffDays < 6) {
//     const d1 = new Date(date1End);
//     const d2 = new Date(date2Start);
//     let testDate = new Date();
//     let allNonWorking = true;
//     // Start from the day after date 1
//     testDate = addDaysToDate(d1, 1);
//     while (testDate.getTime() < d2.getTime()) {
//       if (!isNonWorkingDay(testDate)) {
//         allNonWorking = false;
//         break;
//       }
//       // Increment date
//       testDate = addDaysToDate(testDate, 1);
//     }
//     return allNonWorking;
//   }
//   return false;
// };

enum DayOfWeek {
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
  Saturday = 6,
  Sunday = 7,
}

export const getISODayOfWeek = (date: Date): DayOfWeek => {
  const dayOfWeek = date.getDay();

  if (dayOfWeek === 0) return DayOfWeek.Sunday;
  return dayOfWeek;
};

export const isWeekendDay = (date: Date): boolean => {
  return (
    getISODayOfWeek(date) === DayOfWeek.Saturday ||
    getISODayOfWeek(date) === DayOfWeek.Sunday
  );
};
export const formattedDate = (date: Date, format: "dd/mm/yyyy"): string => {
  if (format === "dd/mm/yyyy") {
    return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
  } else {
    return date.toISOString().substring(0, 10);
  }
};
export const getDateStringsInRange = (start: Date, end: Date) => {
  // Difference in ms divided by ms/day to get days. Add 1 to get inclusive date range
  const numberOfDaysInRange =
    Math.round((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;
  const arr = [];

  for (let days = 1; days <= numberOfDaysInRange; days++) {
    const newDate = new Date(start.getTime() + days * 1000 * 60 * 60 * 24);
    arr.push(newDate.toISOString().substring(0, 10));
  }

  return arr;
};

export const getWorkingDaysInRange = (
  startDate: Date | string,
  endDate: Date | string,
  isNonWorkingDay: (date: Date) => boolean,
  inclusive: boolean = true
): number => {
  let workingDays = 0;

  const startDateObj = parseAndValidateDate(startDate, "Start date");
  const endDateObj = parseAndValidateDate(endDate, "End date");

  let currentDate = inclusive ? startDateObj : addDaysToDate(startDateObj, 1);

  while (isDateBefore(currentDate, endDateObj)) {
    if (!isNonWorkingDay(currentDate)) {
      workingDays++;
    }
    currentDate = addDaysToDate(currentDate, 1);
  }
  if (inclusive && !isNonWorkingDay(endDateObj)) {
    workingDays++;
  }

  return workingDays;
};

export const getCompletedWorkingDaysSinceMonthStart = (
  isNonWorkingDay: (date: Date) => boolean,
  referenceDate?: Date
): number => {
  const relDate = referenceDate ? referenceDate : new Date();
  const monthStart = get1stOfMonth(relDate);
  let workingDays = 0;
  let curDate = monthStart;
  while (curDate < relDate) {
    if (!isNonWorkingDay(curDate)) {
      workingDays++;
    }
    curDate = addDaysToDate(curDate, 1);
  }
  return workingDays;
};

export const isEndOfWorkingWeek = (
  isNonWorkingDay: (date: Date) => boolean,
  referenceDate?: Date
): boolean => {
  //Is it the weekend or bank holiday Friday?
  const relDate = referenceDate ? new Date(referenceDate) : new Date();
  return relDate.getDay() === 5
    ? !isNonWorkingDay(relDate)
    : relDate.getDay() === 6 || relDate.getDay() === 0;
};

export const addCompletedWorkingDaysToDate = (
  referenceDate: Date,
  days: number,
  isNonWorkingDay: (date: Date) => boolean
): Date => {
  const direction = days >= 0 ? 1 : -1;
  let workingDays = 0;
  let curDate = new Date(referenceDate);
  const dayLim = days * direction;
  while (workingDays < dayLim) {
    if (!isNonWorkingDay(curDate)) {
      workingDays++;
    }
    curDate = addDaysToDate(curDate, direction);
  }
  return curDate;
};

export const getDateTimeStamp = (
  date: Date,
  time: "Morning" | "Midday" | "EOD"
) => {
  const dateCopy = new Date(date);
  if (time === "Morning") {
    dateCopy.setHours(9, 0, 0, 0);
  } else if (time === "Midday") {
    dateCopy.setHours(13, 15, 0, 0);
  } else if (time === "EOD") {
    dateCopy.setHours(17, 30, 0, 0);
  }

  return dateCopy;
};

export const formatLastEditedDate = (date: Date) => {
  const today = new Date();
  const yesterday = new Date();

  yesterday.setDate(today.getDate() - 1);

  if (isDateEqual(date, today)) {
    return "Edited Today";
  }

  if (isDateEqual(date, yesterday)) {
    return "Edited Yesterday";
  }

  return prettyDateWithYear(date);
};

export const formatHowLongAgoString = (date: Date) => {
  const today = new Date();
  if (today.getTime() < date.getTime()) return "";
  else if (today.getFullYear() - date.getFullYear() >= 1) {
    return "Over a year ago";
  } else if (
    date.getFullYear() === today.getFullYear() &&
    date.getMonth() === today.getMonth() &&
    date.getDate() === today.getDate()
  ) {
    return "Today";
  } else if (
    date.getFullYear() === today.getFullYear() &&
    date.getMonth() === today.getMonth() &&
    date.getDate() === today.getDate() - 1
  ) {
    return "Yesterday";
  } else if (date.getMonth() !== today.getMonth()) {
    const monthDiff = today.getMonth() - date.getMonth();
    return ` ${monthDiff === 1 ? "last month" : monthDiff + "months ago"} `;
  } else {
    return `${today.getDate() - date.getDate()} days ago`;
  }
};
