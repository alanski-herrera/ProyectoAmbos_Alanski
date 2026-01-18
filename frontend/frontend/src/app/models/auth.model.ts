export interface LoginRequest {
  email: string;
  contrasena: string;
}

export interface LoginResponse {
  idAdmin: number;
  nombreAdmin: string;
  email: string;
  token: string;
}

export interface AdminUser {
  idAdmin: number;
  nombreAdmin: string;
  email: string;
}