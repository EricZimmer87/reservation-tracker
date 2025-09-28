import { Routes, Route } from "react-router";
import HomeView from './views/home/HomeView';
import LoginView from "./views/login/LoginView.tsx";
import DashboardView from "./views/dashboard/DashboardView.tsx";

function AppRoutes() {
  return (
    <>
      <Routes>
        <Route path="/" element={<HomeView />} />
        <Route path="/login" element={<LoginView />} />
        <Route path="/dashboard" element={<DashboardView />} />
      </Routes>
    </>
  );
}

export default AppRoutes;