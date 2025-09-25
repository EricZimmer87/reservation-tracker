package com.ejzimmer.reservation_tracker_backend.dto;

import com.ejzimmer.reservation_tracker_backend.model.User;

public class UserDTO {
    private Integer userId;
    private Boolean isAdmin;
    private String username;
    private String email;
    private String passwordHash;
    private Boolean isBanned;

    public UserDTO (User user) {
        this.userId = userId;
        this.isAdmin = isAdmin;
        this.username = username;
        this.email = email;
        this.passwordHash = passwordHash;
        this.isBanned = isBanned;
    }

    // Getters
    public Integer getUserId() { return userId; }
    public Boolean getIsAdmin() { return isAdmin; }
    public String getUsername() { return username; }
    public String getEmail() { return email; }
    public String getPasswordHash() { return passwordHash; }
    public Boolean getIsBanned() { return isBanned; }
}
