package com.pacman.service;

import com.pacman.entity.TetrisLeaderboard;
import com.pacman.entity.User;
import com.pacman.repository.TetrisLeaderboardRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class TetrisLeaderboardService {
    @Autowired
    private TetrisLeaderboardRepository tetrisLeaderboardRepository;
    
    // 提交分数
    public TetrisLeaderboard submitScore(User user, int score, String deviceType) {
        // 检查用户是否已有记录
        Optional<TetrisLeaderboard> existingScore = tetrisLeaderboardRepository.findFirstByUserIdOrderByScoreDesc(user.getId());
        
        TetrisLeaderboard leaderboard;
        if (existingScore.isPresent()) {
            // 如果已有记录，且新分数更高，则更新
            leaderboard = existingScore.get();
            if (score > leaderboard.getScore()) {
                leaderboard.setScore(score);
                leaderboard.setDeviceType(deviceType);
            }
        } else {
            // 否则创建新记录
            leaderboard = new TetrisLeaderboard();
            leaderboard.setUser(user);
            leaderboard.setScore(score);
            leaderboard.setDeviceType(deviceType);
        }
        
        return tetrisLeaderboardRepository.save(leaderboard);
    }
    
    // 获取排行榜
    public List<TetrisLeaderboard> getLeaderboard(int limit) {
        List<TetrisLeaderboard> allScores = tetrisLeaderboardRepository.findAllByOrderByScoreDesc();
        if (limit > 0 && limit < allScores.size()) {
            return allScores.subList(0, limit);
        }
        return allScores;
    }
    
    // 获取用户的最佳成绩
    public Optional<TetrisLeaderboard> getUserBestScore(Long userId) {
        return tetrisLeaderboardRepository.findFirstByUserIdOrderByScoreDesc(userId);
    }
    
    // 获取排名
    public int getRank(int score) {
        Integer rank = tetrisLeaderboardRepository.getRank(score);
        return rank != null ? rank : 1;
    }
}