package com.ejzimmer.reservation_tracker_backend.service;

import com.ejzimmer.reservation_tracker_backend.dto.UserDTO;
import org.springframework.security.oauth2.core.user.OAuth2User;

import java.util.List;

public interface UserService {
    List<UserDTO> getAllUserDTOs();
    UserDTO getCurrentUser(OAuth2User oauthUser);
}
