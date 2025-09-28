import { Link } from "react-router";
import { useAuth } from "../../auth/useAuth.ts";

function Navbar() {
  const { user, loading } = useAuth();

  if (loading) return null;

  return (
    <>
      <nav>
        <Link to="/">Home</Link>
        {user ? (
          <>
            <Link to="/dashboard">Dashboard</Link>
            <span>Welcome, {user.displayName}</span>
          </>
        ) : (
          <Link to="/login">Login</Link>
        )}
      </nav>
    </>
    )
}

export default Navbar;