import {useEffect, useState} from "react";
import type {User} from "../../types/User";

function DashboardView() {
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    fetch("http://localhost:8080/api/users/me", { credentials: "include" })
      .then(res => {
        if (res.status === 401) {
          throw new Error("Unauthorized");
        }
        return res.json();
      })
      .then((data: User) => setUser(data))
      .catch(err => console.error(err));
  }, []);

  if (!user) return <p>Not logged in</p>;

  return (
    <div>
      <h1>Welcome, {user.displayName}</h1>
      <p>Email: {user.email}</p>
      {user.picture && <img src={user.picture} alt="Profile" />}
    </div>
  );
}

export default DashboardView;