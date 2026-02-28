package com.pacman.service;

import com.pacman.entity.User;
import com.pacman.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.Optional;

@Service
public class UserService {
    @Autowired
    private UserRepository userRepository;
    
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();
    
    // 注册用户
    public User register(String username, String account, String password, String securityQuestion, String securityAnswer) {
        // 检查账号是否已存在
        if (userRepository.findByAccount(account).isPresent()) {
            throw new RuntimeException("账号已存在");
        }
        
        // 检查用户名是否已存在
        if (userRepository.findByUsername(username).isPresent()) {
            throw new RuntimeException("用户名已存在");
        }
        
        // 创建新用户
        User user = new User();
        user.setUsername(username);
        user.setAccount(account);
        user.setPassword(passwordEncoder.encode(password));
        user.setSecurityQuestion(securityQuestion);
        user.setSecurityAnswer(passwordEncoder.encode(securityAnswer));
        
        return userRepository.save(user);
    }
    
    // 登录用户
    public Optional<User> login(String account, String password) {
        Optional<User> userOptional = userRepository.findByAccount(account);
        if (userOptional.isPresent()) {
            User user = userOptional.get();
            if (passwordEncoder.matches(password, user.getPassword())) {
                return Optional.of(user);
            }
        }
        return Optional.empty();
    }
    
    // 根据账号查找用户
    public Optional<User> findByAccount(String account) {
        return userRepository.findByAccount(account);
    }
    
    // 根据ID查找用户
    public Optional<User> findById(Long id) {
        return userRepository.findById(id);
    }
    
    // 重置密码
    public void resetPassword(String account, String securityAnswer, String newPassword) {
        User user = userRepository.findByAccount(account)
                .orElseThrow(() -> new RuntimeException("账号不存在"));
        
        if (!passwordEncoder.matches(securityAnswer, user.getSecurityAnswer())) {
            throw new RuntimeException("密保答案错误");
        }
        
        user.setPassword(passwordEncoder.encode(newPassword));
        userRepository.save(user);
    }
    
    // 修改用户名
    public void updateUsername(Long userId, String newUsername) {
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new RuntimeException("用户不存在"));
        
        // 如果用户提交的是自己当前的用户名，直接返回成功，不进行任何数据改动
        if (user.getUsername().equals(newUsername)) {
            return;
        }
        
        // 检查用户名是否已存在
        if (userRepository.findByUsername(newUsername).isPresent()) {
            throw new RuntimeException("用户名已存在");
        }
        
        user.setUsername(newUsername);
        userRepository.save(user);
    }
}
