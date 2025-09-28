import { BrowserRouter } from "react-router";
import Navbar from "./components/navbar/Navbar.tsx";
import AppRoutes from "./AppRoutes.tsx";

export default function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <AppRoutes />
    </BrowserRouter>
  );
}
