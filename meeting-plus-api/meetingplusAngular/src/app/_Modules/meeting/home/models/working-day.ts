
export class WorkingDay {
  dayName: string;
  date: string;
  isActive: boolean;
  constructor(dayName, date) {
    this.dayName = dayName;
    this.date = date;
    this.isActive = false;//We select one and only one tab to be selected
  }
}
export const weekDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
