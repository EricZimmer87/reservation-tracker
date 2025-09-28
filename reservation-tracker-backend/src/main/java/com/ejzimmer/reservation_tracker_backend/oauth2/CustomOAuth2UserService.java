package com.ejzimmer.reservation_tracker_backend.oauth2;

import com.ejzimmer.reservation_tracker_backend.repository.UserRepository;
import org.springframework.security.oauth2.client.userinfo.DefaultOAuth2UserService;
import org.springframework.security.oauth2.client.userinfo.OAuth2UserRequest;
import org.springframework.security.oauth2.client.userinfo.OAuth2UserService;
import org.springframework.security.oauth2.core.user.OAuth2User;
import org.springframework.stereotype.Service;

@Service
public class CustomOAuth2UserService implements OAuth2UserService<OAuth2UserRequest, OAuth2User> {
    private final UserRepository userRepository;
    private final DefaultOAuth2UserService delegate = new DefaultOAuth2UserService();

    public CustomOAuth2UserService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Override
    public OAuth2User loadUser(OAuth2UserRequest userRequest) {
        OAuth2User oauthUser = delegate.loadUser(userRequest);

        String googleId = oauthUser.getAttribute("sub");
        String email = oauthUser.getAttribute("email");
        String name = oauthUser.getAttribute("name");
        String picture = oauthUser.getAttribute("picture");

        return userRepository.findByEmail(email)
                .map(existingUser -> {
                    // First login: no googleId set yet
                    if (existingUser.getGoogleId() == null) {
                        existingUser.setGoogleId(googleId);
                    }
                    // Always update fields in case Google profile changed
                    existingUser.setDisplayName(name);
                    existingUser.setPicture(picture);

                    userRepository.save(existingUser);
                    return oauthUser; // allow login
                })
                .orElseThrow(() ->
                        new RuntimeException("Unauthorized: email not registered in the system")
                );
    }
}
