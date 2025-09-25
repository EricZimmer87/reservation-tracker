package com.ejzimmer.reservation_tracker_backend.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import com.ejzimmer.reservation_tracker_backend.model.User;

public interface UserRepository extends JpaRepository<User, Integer> {
}
