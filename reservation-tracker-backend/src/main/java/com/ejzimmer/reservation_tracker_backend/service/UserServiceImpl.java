package com.ejzimmer.reservation_tracker_backend.service;

import com.ejzimmer.reservation_tracker_backend.dto.UserDTO;
import com.ejzimmer.reservation_tracker_backend.repository.UserRepository;
import org.springframework.security.oauth2.core.user.OAuth2User;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class UserServiceImpl implements UserService{
    private final UserRepository userRepository;

    public UserServiceImpl(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    // Get all UserDTOs
    @Override
    @Transactional(readOnly = true)
    public List<UserDTO> getAllUserDTOs() {
        return userRepository.findAll()
                .stream()
                .map(UserDTO::new)
                .toList();
    }

    @Override
    public UserDTO getCurrentUser(OAuth2User oauthUser) {
        if (oauthUser == null) {
            throw new RuntimeException("No authenticated user found");
        }

        String email = oauthUser.getAttribute("email");

        return userRepository.findByEmail(email)
                .map(UserDTO::new)
                .orElseThrow(() -> new RuntimeException("User not found in database"));
    }
}
