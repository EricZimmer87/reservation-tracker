import { BrowserRouter } from "react-router";
import Navbar from "./components/navbar/Navbar.tsx";
import AppRoutes from "./AppRoutes.tsx";
import Footer from "./components/footer/Footer.tsx";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min";

export default function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <AppRoutes />
      <Footer />
    </BrowserRouter>
  );
}
