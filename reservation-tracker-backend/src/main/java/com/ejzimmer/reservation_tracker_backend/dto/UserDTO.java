package com.ejzimmer.reservation_tracker_backend.dto;

import com.ejzimmer.reservation_tracker_backend.model.User;

public class UserDTO {
    private Long userId;
    private Boolean isAdmin;
    private String email;
    private String displayName;
    private String picture;
    private Boolean isBanned;

    public UserDTO(User user) {
        this.userId = user.getUserId();
        this.isAdmin = user.getIsAdmin();
        this.email = user.getEmail();
        this.displayName = user.getDisplayName();
        this.picture = user.getPicture();
        this.isBanned = user.getIsBanned();
    }

    // Getters
    public Long getUserId() { return userId; }
    public Boolean getIsAdmin() { return isAdmin; }
    public String getEmail() { return email; }
    public String getDisplayName() { return displayName; }
    public String getPicture() { return picture; }
    public Boolean getIsBanned() { return isBanned; }
}