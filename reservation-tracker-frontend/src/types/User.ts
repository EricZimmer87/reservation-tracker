export interface User {
  userId: number;
  isAdmin: boolean;
  email: string;
  displayName: string;
  picture?: string;
  isBanned: boolean;
}