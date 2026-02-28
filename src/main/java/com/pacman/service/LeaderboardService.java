package com.pacman.service;

import com.pacman.entity.Leaderboard;
import com.pacman.entity.User;
import com.pacman.repository.LeaderboardRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class LeaderboardService {
    @Autowired
    private LeaderboardRepository leaderboardRepository;
    
    // 提交分数
    public Leaderboard submitScore(User user, int score, int timeUsed, String deviceType) {
        Leaderboard leaderboard = new Leaderboard();
        leaderboard.setUser(user);
        leaderboard.setScore(score);
        leaderboard.setTimeUsed(timeUsed);
        leaderboard.setDeviceType(deviceType);
        
        return leaderboardRepository.save(leaderboard);
    }
    
    // 获取排行榜
    public List<Leaderboard> getLeaderboard(int limit) {
        List<Leaderboard> allScores = leaderboardRepository.findAllByOrderByScoreDescTimeUsedAsc();
        if (limit > 0 && limit < allScores.size()) {
            return allScores.subList(0, limit);
        }
        return allScores;
    }
    
    // 获取用户的最佳成绩
    public Optional<Leaderboard> getUserBestScore(Long userId) {
        return leaderboardRepository.findFirstByUserIdOrderByScoreDescTimeUsedAsc(userId);
    }
    
    // 获取排名
    public int getRank(int timeUsed, int score) {
        Integer rank = leaderboardRepository.getRank(timeUsed, score);
        return rank != null ? rank : 1;
    }
}
